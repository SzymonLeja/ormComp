package com.ormComp.controller;

import com.ormComp.service.UserService;
import com.ormComp.model.User;
import lombok.AllArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.repository.query.Param;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.time.OffsetDateTime;
import java.util.List;

@RestController
public class FrontEndController {
    private final UserService userService;

    @Autowired
    public FrontEndController(UserService userService) {
        this.userService = userService;
    }

    @GetMapping("/findAvailableFlats")
    public String findAvailableFlats(@Param("startDate") String startDate, @Param("endDate") String endDate, @Param("price") double price, @Param("capacity") short capacity){
        return userService.findAvailableFlats(OffsetDateTime.parse(startDate.replace(" ", "+")), OffsetDateTime.parse(endDate.replace(" ", "+")), price, capacity).toString();
    }
    @GetMapping("/findFlatsNearLocation")
    public String findFlatsNearLocation(@Param("longitude") double longitude, @Param("latitude") double latitude, @Param("distance") double distance){
        return userService.findFlatsNearLocation(longitude, latitude, distance).toString();
    }
    @GetMapping("/findPopularFlatsStatistics")
    public String findPopularFlatsStatistics(){
        return userService.findPopularFlatsStatistics().toString();
    }
    @GetMapping("/findHighestEarningFlatByOwner")
    public String findHighestEarningFlatByOwner(@Param("ownerId") Long ownerId, @Param("startDate") String startDate, @Param("endDate") String endDate){
        return userService.findHighestEarningFlatByOwner(ownerId, OffsetDateTime.parse(startDate.replace(" ", "+")), OffsetDateTime.parse(endDate.replace(" ", "+"))).toString();
    }
    @GetMapping("/findMostPopularAndProfitableFlats")
    public String findMostPopularAndProfitableFlats(){
        return userService.findMostPopularAndProfitableFlats().toString();
    }
    @GetMapping("/findReservationsByUserId")
    public String findReservationsByUserId(@Param("id") Long id){
        return userService.findReservationsByUserId(id).toString();
    }
    @GetMapping("/findByCityAndNumberOfFlats")
    public String findByCityAndNumberOfFlats(@Param("city") String city, @Param("numberOfFlats") long numberOfFlats){
        return userService.findByCityAndNumberOfFlats(city, numberOfFlats).toString();
    }

}
