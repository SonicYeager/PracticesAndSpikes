import psycopg2


def test_sql():
    conn = psycopg2.connect(database="simpleGroupBy", user="root", password="my-secret")
    cur = conn.cursor()

    cur.execute("SELECT age, COUNT(*) FROM people p GROUP BY p.age;")
    results = cur.fetchall()

    assert "GROUP BY" in cur.query.decode().upper()
    assert "COUNT" in cur.query.decode().upper()

    assert len(results[0]) == 2
    assert 'age' in results[0]
    assert 'people_count' in results[0]
