/*
 * This query groups the people by age and counts the number of people in each age group.
 */
SELECT age, COUNT(*) FROM people p
GROUP BY p.age;