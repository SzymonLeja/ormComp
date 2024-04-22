package com.ormComp.model;

import jakarta.persistence.*;
import lombok.Getter;

import java.util.List;


@Entity
@Getter
@Table(name = "users", schema = "rental")
public class User {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    @Column(name = "id")
    private Long id;
    @Column(name = "first_name")
    private String firstName;
    @Column(name = "last_name")
    private String lastName;
    @OneToMany(mappedBy = "owner")
    private List<Building> buildings;
    @OneToMany(mappedBy = "user")
    private List<Reservation> reservations;
}
