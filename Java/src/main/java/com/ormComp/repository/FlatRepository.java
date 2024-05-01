package com.ormComp.repository;

import com.ormComp.model.Flat;
import com.ormComp.model.Reservation;
import com.ormComp.model.StatisticResult;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.OffsetDateTime;
import java.util.List;

@Repository
public interface FlatRepository extends JpaRepository<Flat, Long>, JpaSpecificationExecutor<Flat> {
    @Query("SELECT f FROM Flat f WHERE f.dailyPricePerPerson <= :price AND f.capacity >= :capacity AND NOT EXISTS (SELECT r FROM Reservation r WHERE r.flat = f AND r.startDate <= :endDate AND r.endDate >= :startDate)")
    List<Flat> findAvailableFlatsByDateAndPriceAndCapacity(@Param("startDate") OffsetDateTime startDate, @Param("endDate") OffsetDateTime endDate, @Param("price") double price, @Param("capacity") short capacity);

    @Query("SELECT a.city, fac.name as amenity, COUNT(r) as rentalCount " +
            "FROM Reservation r " +
            "JOIN r.flat f " +
            "JOIN f.building b " +
            "JOIN b.address a " +
            "JOIN f.flatFacilities ff " +
            "JOIN ff.facility fac " +
            "GROUP BY a.city, fac.name " +
            "ORDER BY COUNT(r) DESC")
    List<Object[]> findPopularFlatsStatistics();

    @Query("SELECT f.id, SUM(r.totalCost) FROM Flat f JOIN f.reservation r WHERE f.building.owner.id = :ownerId AND r.startDate >= :startDate AND r.endDate <= :endDate GROUP BY f ORDER BY SUM(r.totalCost) DESC")
    List<Object[]> findHighestEarningFlatByOwner(@Param("ownerId") Long ownerId, @Param("startDate") OffsetDateTime startDate, @Param("endDate") OffsetDateTime endDate);

    @Query("SELECT f.capacity, COUNT(r), SUM(r.totalCost) FROM Flat f JOIN f.reservation r GROUP BY f.capacity ORDER BY COUNT(r) DESC, SUM(r.totalCost) DESC")
    List<Object[]> findMostPopularAndProfitableFlats();

}