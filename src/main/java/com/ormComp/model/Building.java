package com.ormComp.model;

import jakarta.persistence.*;
import lombok.Data;
import lombok.Getter;

import java.util.List;
import java.util.Objects;

@Entity
@Getter
@Table(name = "building", schema = "rental")
public class Building {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    private long id;
    @Column(name = "description")
    private String description;
    @ManyToOne
    @JoinColumn(name = "owner_id")
    private User owner;
    @ManyToOne
    @JoinColumn(name = "address_id")
    private Address address;
    @OneToMany(mappedBy = "building")
    private List<Flat> flat;
}
