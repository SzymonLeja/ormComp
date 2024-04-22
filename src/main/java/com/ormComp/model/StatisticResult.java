package com.ormComp.model;

public class StatisticResult {
    private String city;
    private String name;
    private long count;

    public StatisticResult(String city, String name, long count) {
        this.city = city;
        this.name = name;
        this.count = count;
    }

    public String getName() {
        return name;
    }

    public String getCity() {
        return city;
    }

    public long getCount() {
        return count;
    }

}
