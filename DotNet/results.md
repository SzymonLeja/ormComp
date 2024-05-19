## GetUsersWithFlatNumberInCity

Raw SQL:

```
SELECT u.id AS Id, u.first_name AS FirstName, u.last_name AS LastName
FROM rental.users u
JOIN rental.building b ON u.id = b.owner_id
JOIN rental.flats f ON b.id = f.building_id
JOIN rental.addresses a ON b.address_id = a.id
WHERE a.city = @city
GROUP BY u.id
HAVING COUNT(f.id) = @flatNumber;
```

## GetUsersWithFlatNumberInCity

Generated SQL:

```
-- @__city_0='Langworthhaven'
-- @__flatNumber_1='5'
SELECT u.id AS "Id", u.first_name AS "FirstName", u.last_name AS "LastName"
FROM rental.users AS u
WHERE (
    SELECT count(*)::int
    FROM rental.building AS b
    INNER JOIN rental.addresses AS a ON b.address_id = a.id
    INNER JOIN rental.flats AS f ON b.id = f.building_id
    WHERE u.id = b.owner_id AND a.city = @__city_0) = @__flatNumber_1
```

## GetUserReservations

Raw SQL:

```
SELECT r.id AS Id, r.start_date AS StartDate, r.end_date AS EndDate, r.guest_number AS GuestNumber, r.total_cost AS TotalCost
            FROM rental.reservations r
            WHERE r.reserved_by_id = @userId;
```

## GetUserReservations

Generated SQL:

```
-- @__userId_0='1'
SELECT r.id AS "Id", r.start_date AS "StartDate", r.end_date AS "EndDate", r.guest_number AS "GuestNumber", r.total_cost AS "TotalCost"
FROM rental.reservations AS r
WHERE r.reserved_by_id = @__userId_0
```

## GetUserFlatWithHighestIncome

Generated SQL:

```
SELECT f.id AS Id, SUM(r.total_cost) AS TotalRevenue
FROM rental.flats f
JOIN rental.reservations r ON f.id = r.flat_id
WHERE r.start_date >= @startDate AND r.end_date <= @endDate
AND f.building_id IN (SELECT id FROM rental.building WHERE owner_id = @userId) 
GROUP BY f.id
ORDER BY TotalRevenue DESC
LIMIT 1;
```

## GetUserFlatWithHighestIncome

Generated SQL:

```
-- @__ToUniversalTime_1='2023-12-31T23:00:00.0000000Z' (DbType = DateTime)
-- @__ToUniversalTime_2='2024-05-30T22:00:00.0000000Z' (DbType = DateTime)
-- @__userId_0='4'
SELECT f.id AS "Id", (
    SELECT COALESCE(sum(r0.total_cost), 0)
    FROM rental.reservations AS r0
    WHERE f.id = r0.flat_id AND r0.start_date >= @__ToUniversalTime_1 AND r0.end_date <= @__ToUniversalTime_2) AS "TotalRevenue"
FROM rental.flats AS f
INNER JOIN rental.building AS b ON f.building_id = b.id
WHERE b.owner_id = @__userId_0
ORDER BY (
    SELECT COALESCE(sum(r.total_cost), 0)
    FROM rental.reservations AS r
    WHERE f.id = r.flat_id AND r.start_date >= @__ToUniversalTime_1 AND r.end_date <= @__ToUniversalTime_2) DESC
```

## GetMostPopularFlatStats

Raw SQL:

```
SELECT fac.name AS Amenity, COUNT(*) AS RentalCount
FROM rental.reservations r
JOIN rental.flats f ON r.flat_id = f.id
JOIN rental.building b ON f.building_id = b.id
JOIN rental.addresses a ON b.address_id = a.id
JOIN rental.flat_facility ff ON f.id = ff.flat_id
JOIN rental.facilities fac ON ff.facility_id = fac.id
WHERE a.city = @city
GROUP BY fac.name
ORDER BY RentalCount DESC;
```

## GetMostPopularFlatStats

Generated SQL:

```
-- @__city_0='Langworthhaven'
SELECT t.name AS "Amenity", count(*)::int AS "RentalCount"
FROM rental.reservations AS r
INNER JOIN rental.flats AS f ON r.flat_id = f.id
INNER JOIN rental.building AS b ON f.building_id = b.id
INNER JOIN rental.addresses AS a ON b.address_id = a.id
INNER JOIN (
    SELECT f1.name, f0.flat_id
    FROM rental.flat_facility AS f0
    INNER JOIN rental.facilities AS f1 ON f0.facility_id = f1.id
) AS t ON f.id = t.flat_id
WHERE a.city = @__city_0
GROUP BY t.name
ORDER BY count(*)::int DESC
```

## GetFlatsNearLocation

Raw SQL:

```
SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, f.building_id AS BuildingId, f.flat_number AS FlatNumber
FROM rental.flats f
JOIN rental.building b ON f.building_id = b.id
JOIN rental.addresses a ON b.address_id = a.id
WHERE (
    6371 * acos (
        cos(radians(@latitude)) * cos(radians(a.latitude)) * cos(radians(a.longitude - @longitude)) +
        sin(radians(@latitude)) * sin(radians(a.latitude))
    )
) < @radius;
```

## GetFlatsNearLocation

Generated SQL:

```
-- @__Cos_0='0.6156614753256583'
-- @__p_1='0.3839724354387525'
-- @__Sin_2='0.788010753606722'
-- @__radius_3='0.1'
SELECT f.id AS "Id", f.description AS "Description", f.daily_price_per_person AS "DailyPricePerPerson", f.capacity AS "Capacity", f.building_id AS "BuildingId", f.flat_number AS "FlatNumber"
FROM rental.flats AS f
INNER JOIN rental.building AS b ON f.building_id = b.id
INNER JOIN rental.addresses AS a ON b.address_id = a.id
WHERE 6371.0 * acos((@__Cos_0 * cos((3.1415926535897931 * a.latitude::double precision) / 180.0)) * cos((3.1415926535897931 * a.longitude::double precision) / 180.0 - @__p_1) + @__Sin_2 * sin((3.1415926535897931 * a.latitude::double precision) / 180.0)) < @__radius_3
```

## GetFlatsByQuery

Raw SQL:

```
SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, b.id AS BuildingId, f.flat_number AS FlatNumber
FROM rental.flats f
JOIN rental.building b ON f.building_id = b.id
WHERE f.daily_price_per_person <= @maxPrice
AND f.capacity >= @minCapacity
AND NOT EXISTS (
    SELECT 1 FROM rental.reservations r
    WHERE r.flat_id = f.id
    AND (r.start_date, r.end_date) OVERLAPS (@startDate, @endDate)
)
ORDER BY f.id;
```

## GetFlatsByQuery

Generated SQL:

```
-- @__maxPrice_0='10'
-- @__minCapacity_1='9' (DbType = Int16)
-- @__startDateUtc_2='2024-01-01T00:00:00.0000000Z' (DbType = DateTime)
-- @__endDateUtc_3='2024-01-31T00:00:00.0000000Z' (DbType = DateTime)
SELECT f.id AS "Id", f.description AS "Description", f.daily_price_per_person AS "DailyPricePerPerson", f.capacity AS "Capacity", f.building_id AS "BuildingId", f.flat_number AS "FlatNumber"
FROM rental.flats AS f
WHERE f.daily_price_per_person <= @__maxPrice_0 AND f.capacity >= @__minCapacity_1 AND NOT EXISTS (
    SELECT 1
    FROM rental.reservations AS r
    WHERE f.id = r.flat_id AND ((r.start_date <= @__startDateUtc_2 AND r.end_date >= @__endDateUtc_3) OR (r.start_date >= @__startDateUtc_2 AND r.start_date < @__endDateUtc_3) OR (r.end_date > @__startDateUtc_2 AND r.end_date <= @__endDateUtc_3)))
ORDER BY f.id
```

## GetFlatsByCapacityAndRevenue

Raw SQL:

```
SELECT f.capacity AS Capacity, COUNT(r.id) AS NumberOfRentals, SUM(r.total_cost) AS TotalRevenue
FROM rental.flats f
JOIN rental.reservations r ON f.id = r.flat_id
GROUP BY f.capacity
ORDER BY NumberOfRentals DESC, TotalRevenue DESC;
```

## GetFlatsByCapacityAndRevenue

Generated SQL:

```
SELECT f.capacity AS "Capacity", (
    SELECT count(*)::int
    FROM rental.flats AS f1
    INNER JOIN rental.reservations AS r1 ON f1.id = r1.flat_id
    WHERE f.capacity = f1.capacity)::bigint AS "NumberOfRentals", COALESCE(sum((
    SELECT COALESCE(sum(r2.total_cost), 0)
    FROM rental.reservations AS r2
    WHERE f.id = r2.flat_id)), 0) AS "TotalRevenue"
FROM rental.flats AS f
GROUP BY f.capacity
ORDER BY (
    SELECT count(*)::int
    FROM rental.flats AS f1
    INNER JOIN rental.reservations AS r1 ON f1.id = r1.flat_id
    WHERE f.capacity = f1.capacity)::bigint DESC, COALESCE(sum((
    SELECT COALESCE(sum(r2.total_cost), 0)
    FROM rental.reservations AS r2
    WHERE f.id = r2.flat_id)), 0) DESC
```

