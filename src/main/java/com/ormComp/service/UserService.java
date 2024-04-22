package com.ormComp.service;

import com.ormComp.model.Flat;
import com.ormComp.model.Reservation;
import com.ormComp.model.StatisticResult;
import com.ormComp.model.User;
import com.ormComp.repository.FlatRepository;
import com.ormComp.repository.UserRepository;
import jakarta.persistence.criteria.Join;
import jakarta.persistence.criteria.JoinType;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Component;
import org.springframework.stereotype.Service;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Component
public class UserService {
    private final UserRepository userRepository;

    private final FlatRepository flatRepository;

    @Autowired
    public UserService(UserRepository userRepository, FlatRepository flatRepository) {
        this.userRepository = userRepository;
        this.flatRepository = flatRepository;
    }

    public List<User> findUsers() {
        return userRepository.findAll();
    }


    public List<Flat> findAvailableFlats(OffsetDateTime startDate, OffsetDateTime endDate, double price, short capacity) {
        List<Flat> allFlats = flatRepository.findAvailableFlatsByDateAndPriceAndCapacity(startDate, endDate, price, capacity);
        return  allFlats;
    }
    public List<Flat> findFlatsNearLocation(double longitude, double latitude, double distance) {
        List<Flat> allFlats = flatRepository.findAll();
        return allFlats.stream()
                .filter(f -> calculateDistance(f.getBuilding().getAddress().getLatitude(), f.getBuilding().getAddress().getLatitude(), latitude, longitude) <= distance)
                .collect(Collectors.toList());
    }
    public List<Object[]> findPopularFlatsStatistics() {
        return flatRepository.findPopularFlatsStatistics();
    }
    public List<Object[]> findHighestEarningFlatByOwner(Long ownerId, OffsetDateTime startDate, OffsetDateTime endDate) {
        return flatRepository.findHighestEarningFlatByOwner(ownerId, startDate, endDate);
    }
    public List<Object[]> findMostPopularAndProfitableFlats() {
        return flatRepository.findMostPopularAndProfitableFlats();
    }
    public List<Reservation> findReservationsByUserId(Long id) {
        return userRepository.findById(id).get().getReservations();
    }
    public List<User> findByCityAndNumberOfFlats(String city, long numberOfFlats) {
        return userRepository.findByCityAndNumberOfFlats(city, numberOfFlats);
    }
    private double calculateDistance(double lat1, double lon1, double lat2, double lon2) {
        final int R = 6371;
        double latDistance = Math.toRadians(lat2 - lat1);
        double lonDistance = Math.toRadians(lon2 - lon1);
        double a = Math.sin(latDistance / 2) * Math.sin(latDistance / 2)
                + Math.cos(Math.toRadians(lat1)) * Math.cos(Math.toRadians(lat2))
                * Math.sin(lonDistance / 2) * Math.sin(lonDistance / 2);
        double c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        double distance = R * c;

        return distance;
    }
}
