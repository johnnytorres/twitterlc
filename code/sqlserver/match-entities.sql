

select * from TwitterProfiles 
select * from Comercio$

select * from TwitterProfiles where Name in (Select f1 from Comercio$)
union
select * from TwitterProfiles where Name in (Select f1 from Expreso$)
union
select * from TwitterProfiles where Name in (Select f1 from Universo$)

select distinct p.name, e.Nombre, e.Fuente from TwitterProfiles p
right join Entidades e on e.Nombre =p.Name 