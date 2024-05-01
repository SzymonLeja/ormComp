package com.ormComp.repository;

import com.ormComp.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface UserRepository extends JpaRepository<User, Long>{
    @Query("SELECT u FROM User u JOIN u.buildings b JOIN b.address a WHERE a.city = :city GROUP BY u HAVING COUNT(b) = :numberOfFlats")
    List<User> findByCityAndNumberOfFlats(@Param("city") String city, @Param("numberOfFlats") long numberOfFlats);
}
