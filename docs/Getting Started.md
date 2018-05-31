# DB Setup
Need a local install of PostGres.

Then run this script (from the main postgres db):

```
CREATE USER isnsfw WITH PASSWORD 'IsNSFW';
GRANT ALL PRIVILEGES ON DATABASE isnsfw TO isnsfw;
```

Want some seed Tags?

```
INSERT INTO Tags (Key, Name, short_description, sort_order, is_deleted)
VALUES ('O', 'Offensive', 'Content has a tendancy to offend', 1, false);

INSERT INTO Tags (Key, Name, short_description, sort_order, is_deleted)
VALUES ('G', 'Gore', 'Gory content lies within', 2, false);

INSERT INTO Tags (Key, Name, short_description, sort_order, is_deleted)
VALUES ('N', 'Nudity', 'You might see some body parts', 3, false);

INSERT INTO Tags (Key, Name, short_description, sort_order, is_deleted)
VALUES ('XXX', 'Pornography', 'Not a Vin Diesel movie', 4, false);
```