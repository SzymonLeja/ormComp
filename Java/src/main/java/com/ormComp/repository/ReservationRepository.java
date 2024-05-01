package com.ormComp.repository;

import com.ormComp.model.Reservation;
import com.ormComp.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ReservationRepository extends JpaRepository<Reservation, Long>{

    List<Reservation> findByUserId(Long id);
}
