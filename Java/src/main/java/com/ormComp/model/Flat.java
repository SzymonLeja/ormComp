package com.ormComp.model;

import jakarta.persistence.*;
import lombok.Data;
import lombok.Getter;

import java.util.List;

@Entity
@Getter
@Table(name = "flats", schema = "rental")
public class Flat {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    private long id;
    @Column(name = "description")
    private String description;
    @Column(name = "capacity")
    private short capacity;
    @Column(name = "daily_price_per_person")
    private double dailyPricePerPerson;
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "building_id")
    private Building building;
    @Column(name = "flat_number")
    private String flatNumber;
    @OneToMany(mappedBy = "flat")
    private List<Reservation> reservation;

    @OneToMany(mappedBy = "flat")
    private List<FlatFacility> flatFacilities;

    @Override
    public String toString() {
        return "Flat{" +
                "id=" + id +
                ", description='" + description + '\'' +
                ", capacity=" + capacity +
                ", dailyPricePerPerson=" + dailyPricePerPerson +
                ", building=" + building +
                ", flatNumber='" + flatNumber + '\'' +
                '}';
    }
}
