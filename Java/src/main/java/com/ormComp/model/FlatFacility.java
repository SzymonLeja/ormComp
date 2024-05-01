package com.ormComp.model;

import jakarta.persistence.*;

@Entity
@Table(name = "flat_facility", schema = "rental")
public class FlatFacility {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    private long id;
    @ManyToOne
    @JoinColumn(name = "facility_id")
    private Facilities facility;
    @ManyToOne
    @JoinColumn(name = "flat_id")
    private Flat flat;
}
