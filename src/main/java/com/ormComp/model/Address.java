package com.ormComp.model;

import jakarta.persistence.*;
import lombok.Data;

import java.util.List;

@Entity
@Data
@Table(name = "addresses", schema = "rental")
public class Address {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    private long id;
    @Column(name = "city")
    private String city;
    @Column(name = "street")
    private String street;
    @Column(name = "post_code")
    private String postCode;
    @Column(name = "number")
    private String number;
    @Column(name = "country")
    private String country;
    @Column(name = "longitude")
    private double longitude;
    @Column(name = "latitude")
    private double latitude;
    @OneToMany(mappedBy = "address")
    private List<Building> building;

}
