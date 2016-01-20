

select * from [dbo].[TwitterProfiles]

select YEAR(CreatedAt), count(distinct(CreatedBy)) from TwitterTweets 
group by YEAR(CreatedAt)
order by 1


select tt.CreatedBy
	, max(tp.FavouritesCount) FavouritesCount
	, max(tp.FollowersCount) FollowersCount
	, max(tp.FriendsCount) FriendsCount
	, max(tp.StatusesCount) StatusesCount
	, max(tp.ListedCount) ListedCount
	, sum(tt.RetweetCount) RetweetCount
	, sum(case when text not like 'RT %' then RetweetCount else 0 end) RetweetCountOriginal	
	, sum(tt.FavouriteCount)  FavouriteCount
	, sum(case when text not like 'RT %' then tt.FavouriteCount else 0 end) FavouriteCountOriginal
from TwitterTweets tt
join TwitterProfiles tp on tt.CreatedBy=tp.ScreenName
where
	Year(tt.CreatedAt) > 2014 
	and (lower(tp.Location) like '%ec%' or LOWER(Location) in ('quito','guayaquil','cuenca','machala','ambato'))
group by  tt.CreatedBy
order by 3 desc

select * from TwitterProfiles where Name like '%martha%'
select count(*) from TwitterProfiles 
where LOWER(Location) in ('quito','guayaquil','cuenca','machala','ambato')
--LOWER(Location) like '%guayaquil%'
--or LOWER(Location) like '%cuenca%'
--or LOWER(Location) like '%quito%'
--or LOWER(Location) like '%machala%'
--or LOWER(Location) like '%ambato%'
or  LOWER(Location)   like '%ec%'
--group by Location
order by FollowersCount desc


select count(*) from TwitterProfiles where Location <> ''

select count(*) from TwitterProfiles

select sum(RetweetCount) from TwitterTweets  where CreatedBy = 'jimmyjairala' and Text not like 'RT %'


