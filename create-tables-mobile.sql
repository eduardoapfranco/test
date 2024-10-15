       
CREATE TABLE IF NOT EXISTS categories (
id integer primary key NOT NULL,
title text NOT NULL,
codigo_interno text NULL,
content text,
orderr integer NOT NULL,
parent_id integer NULL,
icon text NULL,
ativo integer,
visible_app integer,
image text,
created_at CURRENT_TIMESTAMP NULL,
updated_at CURRENT_TIMESTAMP NULL);

 CREATE TABLE IF NOT EXISTS checklists (
        id integer primary key NOT NULL,
        title text NOT NULL,
        content text NULL,
        orderr integer NULL,
        type integer NULL,
        category_id int NULL,
        user_id int NULL,
        ativo integer NULL,
        visible_app integer NULL,
        check_enable integer NULL,
        created_at CURRENT_TIMESTAMP NULL,
        updated_at CURRENT_TIMESTAMP NULL)