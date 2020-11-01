create database Music_DB;
go
create table Users(
UserId int primary key identity(1,1),
Login nvarchar(100) not null unique,
Mail nvarchar(100) not null unique,
Password nvarchar(500) not null,
Avatar varbinary(max),
Name nvarchar(100),
Surname nvarchar(100),
Country nvarchar(100),
City nvarchar(100)
);
go