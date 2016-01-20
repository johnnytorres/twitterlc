
-- lista de lideres
select Name, ScreenName, FollowersCount 
from TwitterProfiles 
where Yleader=1
order by FollowersCount desc

-- volumen de actividad por lider
select 
	min(p.ScreenName) as ScreenName, 
	min(p.Name) as Name, 
	min(cast(t.CreatedAt as date)) as MinWeekDate,
	min(cast(t.CreatedAt as date)) as MaxWeekDate,
	count(*) as NumTweets,	
	sum(t.RetweetCount) as NumRetweets,
	sum(t.FavouriteCount) as NumFavourites
from TwitterProfiles p
join TwitterTweets t on p.Id=t.ProfileId
where Yleader=1 
and t.CreatedAt >= '2015-06-01' 
and t.CreatedAt <= '2015-08-23'
and SUBSTRING(Text,0,5) <> 'RT @'
and p.id in (
		select top 10 p.Id
	from TwitterProfiles p 
	join TwitterTweets t on p.Id=t.ProfileId
	where t.CreatedAt >= '2015-06-01' 
	and p.Yleader=1
	group by p.Id
	order by sum(t.RetweetCount+t.FavouriteCount) desc
	)
group by t.CreatedBy
order by ScreenName 


-- volumen de actividad por lider
select 
	min(p.ScreenName) as ScreenName, 
	min(p.Name) as Name, 
	
	DATEPART(Week,t.CreatedAt) as CreatedAtWeek,
	min(cast(t.CreatedAt as date)) as MinWeekDate,
	max(cast(t.CreatedAt as date)) as MaxWeekDate,
	count(*) as NumTweets,	
	sum(t.RetweetCount) as NumRetweets,
	sum(t.FavouriteCount) as NumFavourites
from TwitterProfiles p
join TwitterTweets t on p.Id=t.ProfileId
where Yleader=1
and t.CreatedAt >= '2015-06-01' 
and t.CreatedAt <= '2015-08-23'
and SUBSTRING(Text,0,5) <> 'RT @'
and p.Id in (
		select top 10 p.Id
	from TwitterProfiles p 
	join TwitterTweets t on p.Id=t.ProfileId
	where t.CreatedAt >= '2015-06-01' 
	and p.Yleader=1
	group by p.Id
	order by sum(t.RetweetCount) desc
	)
group by t.CreatedBy, DATEPART(WEEK, t.CreatedAt)--, day(t.CreatedAt)
order by ScreenName, CreatedAtWeek-- NumRetweets desc,


-- volumen de atencion por lider
select 
	min(p.ScreenName) as ScreenName, 
	min(p.Name) as Name, 
	year(t.CreatedAt) as CreatedAtYear, 
	month(t.CreatedAt) as CreatedAtMonth, 
	DATEPART(Week,t.CreatedAt) as CreatedAtWeek,
	min(cast(t.CreatedAt as date)) as MinWeekDate,
	max(cast(t.CreatedAt as date)) as MaxWeekDate,
	count(*) as NumTweets,	
	sum(t.RetweetCount) as NumRetweets
from TwitterProfiles p
join TwitterTweets t on p.Id=t.ProfileId
where t.CreatedAt > '2015-05-31' and p.Id in (
	select top 5 p.Id
	from TwitterProfiles p 
	join TwitterTweets t on p.Id=t.ProfileId
	where t.CreatedAt > '2015-05-31' 
	group by p.Id
	order by sum(t.RetweetCount) desc)
group by t.CreatedBy, year(t.CreatedAt), month(t.CreatedAt), DATEPART(WEEK, t.CreatedAt)--, day(t.CreatedAt)
order by ScreenName, CreatedAtYear, CreatedAtMonth


-- solo tweets originales
select p.Name, p.ScreenName, p.StatusesCount, t.RetweetCount, t.FavouriteCount, t.Id, t.CreatedAt,DATEPART(WEEK, t.CreatedAt) from TwitterTweets t
join TwitterProfiles p on p.Id=t.ProfileId
where p.Yleader=1 
and t.CreatedAt >= '2015-06-01' 
and t.CreatedAt <= '2015-08-23'
and SUBSTRING(Text,0,5) <> 'RT @'