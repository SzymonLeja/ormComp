SELECT u.id AS Id, u.first_name AS FirstName, u.last_name AS LastName
FROM rental.users u
JOIN rental.building b ON u.id = b.owner_id
JOIN rental.flats f ON b.id = f.building_id
JOIN rental.addresses a ON b.address_id = a.id
WHERE a.city = 'Langworthhaven'
GROUP BY u.id
HAVING COUNT(f.id) = 5;

SELECT u.id AS "Id", u.first_name AS "FirstName", u.last_name AS "LastName"
FROM rental.users AS u
WHERE (
    SELECT count(*)::int
    FROM rental.building AS b
    INNER JOIN rental.addresses AS a ON b.address_id = a.id
    INNER JOIN rental.flats AS f ON b.id = f.building_id
    WHERE u.id = b.owner_id AND a.city = 'Langworthhaven') = 5;
	
SELECT r.id AS Id, r.start_date AS StartDate, r.end_date AS EndDate, r.guest_number AS GuestNumber, r.total_cost AS TotalCost
            FROM rental.reservations r
            WHERE r.reserved_by_id = 1;
			
SELECT r.id AS "Id", r.start_date AS "StartDate", r.end_date AS "EndDate", r.guest_number AS "GuestNumber", r.total_cost AS "TotalCost"
FROM rental.reservations AS r
WHERE r.reserved_by_id = 1;
	
SELECT f.id AS Id, SUM(r.total_cost) AS TotalRevenue
FROM rental.flats f
JOIN rental.reservations r ON f.id = r.flat_id
WHERE r.start_date >= '2023-12-31T23:00:00.0000000Z' AND r.end_date <= '2024-05-30T22:00:00.0000000Z'
AND f.building_id IN (SELECT id FROM rental.building WHERE owner_id = 4) 
GROUP BY f.id
ORDER BY TotalRevenue DESC
LIMIT 1;

SELECT f.id AS "Id", (
    SELECT COALESCE(sum(r0.total_cost), 0)
    FROM rental.reservations AS r0
    WHERE f.id = r0.flat_id AND r0.start_date >= '2023-12-31T23:00:00.0000000Z' AND r0.end_date <= '2024-05-30T22:00:00.0000000Z') AS "TotalRevenue"
FROM rental.flats AS f
INNER JOIN rental.building AS b ON f.building_id = b.id
WHERE b.owner_id = 4
ORDER BY (
    SELECT COALESCE(sum(r.total_cost), 0)
    FROM rental.reservations AS r
    WHERE f.id = r.flat_id AND r.start_date >= '2023-12-31T23:00:00.0000000Z' AND r.end_date <= '2024-05-30T22:00:00.0000000Z') DESC;

SELECT fac.name AS Amenity, COUNT(*) AS RentalCount
FROM rental.reservations r
JOIN rental.flats f ON r.flat_id = f.id
JOIN rental.building b ON f.building_id = b.id
JOIN rental.addresses a ON b.address_id = a.id
JOIN rental.flat_facility ff ON f.id = ff.flat_id
JOIN rental.facilities fac ON ff.facility_id = fac.id
WHERE a.city = 'Langworthhaven'
GROUP BY fac.name
ORDER BY RentalCount DESC;

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
WHERE a.city = 'Langworthhaven'
GROUP BY t.name
ORDER BY count(*)::int DESC;

SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, f.building_id AS BuildingId, f.flat_number AS FlatNumber
FROM rental.flats f
JOIN rental.building b ON f.building_id = b.id
JOIN rental.addresses a ON b.address_id = a.id
WHERE (
    6371 * acos (
        cos(radians(52)) * cos(radians(a.latitude)) * cos(radians(a.longitude - 22)) +
        sin(radians(52)) * sin(radians(a.latitude))
    )
) < 0.1;

SELECT f.id AS "Id", f.description AS "Description", f.daily_price_per_person AS "DailyPricePerPerson", f.capacity AS "Capacity", f.building_id AS "BuildingId", f.flat_number AS "FlatNumber"
FROM rental.flats AS f
INNER JOIN rental.building AS b ON f.building_id = b.id
INNER JOIN rental.addresses AS a ON b.address_id = a.id
WHERE 6371.0 * acos((0.6156614753256583 * cos((3.1415926535897931 * a.latitude::double precision) / 180.0)) * cos((3.1415926535897931 * a.longitude::double precision) / 180.0 - 0.3839724354387525) + 0.788010753606722 * sin((3.1415926535897931 * a.latitude::double precision) / 180.0)) < 0.1;

SELECT f.id AS Id, f.description AS Description, f.daily_price_per_person AS DailyPricePerPerson, f.capacity AS Capacity, b.id AS BuildingId, f.flat_number AS FlatNumber
FROM rental.flats f
JOIN rental.building b ON f.building_id = b.id
WHERE f.daily_price_per_person <= 10
AND f.capacity >= 9
AND NOT EXISTS (
    SELECT 1 FROM rental.reservations r
    WHERE r.flat_id = f.id
    AND (r.start_date, r.end_date) OVERLAPS ('2024-01-01T00:00:00.0000000Z', '2024-01-31T00:00:00.0000000Z')
)
ORDER BY f.id;

SELECT f.id AS "Id", f.description AS "Description", f.daily_price_per_person AS "DailyPricePerPerson", f.capacity AS "Capacity", f.building_id AS "BuildingId", f.flat_number AS "FlatNumber"
FROM rental.flats AS f
WHERE f.daily_price_per_person <= 10 AND f.capacity >= 9 AND NOT EXISTS (
    SELECT 1
    FROM rental.reservations AS r
    WHERE f.id = r.flat_id AND ((r.start_date <= '2024-01-01T00:00:00.0000000Z' AND r.end_date >= '2024-01-31T00:00:00.0000000Z') OR (r.start_date >= '2024-01-01T00:00:00.0000000Z' AND r.start_date < '2024-01-31T00:00:00.0000000Z') OR (r.end_date > '2024-01-01T00:00:00.0000000Z' AND r.end_date <= '2024-01-31T00:00:00.0000000Z')))
ORDER BY f.id;

SELECT f.capacity AS Capacity, COUNT(r.id) AS NumberOfRentals, SUM(r.total_cost) AS TotalRevenue
FROM rental.flats f
JOIN rental.reservations r ON f.id = r.flat_id
GROUP BY f.capacity
ORDER BY NumberOfRentals DESC, TotalRevenue DESC;

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
    WHERE f.id = r2.flat_id)), 0) DESC;

COPY (SELECT * FROM pg_stat_statements WHERE queryid in (-2639351766443624826, -6051908327733557471,  5790089386458641856, 5790089386458641856,  -6498272173222603471, -507885921649201663,  3851029712228439361, -4818722781088261692,  2568884615005532003, -6390807139547292995, 3075921541462290673, 1566226529174231237, -417404113517924016, -6456960838080312881)) TO 'put\path\here' DELIMITER ';' CSV HEADER;
