create table public.file_metadata
(
    id            uuid not null
        primary key,
    bucket        varchar(255),
    content_type  varchar(255),
    creation_date date,
    filename      varchar(255),
    object_name   uuid
);

alter table public.file_metadata
    owner to postgres;