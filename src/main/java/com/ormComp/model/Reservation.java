package com.ormComp.model;

import jakarta.persistence.*;

import java.time.OffsetDateTime;
import java.util.Objects;

@Entity
@Table(name = "reservations", schema = "rental")
public class Reservation {
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Id
    @Column(name = "id", nullable = false)
    private long id;
    @Column(name = "start_date", nullable = false)
    private OffsetDateTime startDate;
    @Column(name = "end_date", nullable = false)
    private OffsetDateTime endDate;
    @Column(name = "guest_number", nullable = false)
    private short guestNumber;
    @Column(name = "total_cost", nullable = false)
    private int totalCost;
    @ManyToOne
    @JoinColumn(name = "flat_id")
    private Flat flat;
    @ManyToOne
    @JoinColumn(name = "reserved_by_id")
    private User user;
}
