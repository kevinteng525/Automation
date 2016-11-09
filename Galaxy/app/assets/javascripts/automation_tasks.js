//local format "2010-01-01 10:10"
function localToUTC(local)
{
	var date = local.split(' ')[0];
	var time = local.split(' ')[1];
	var year = Number(date.split('-')[0]);
	var month = Number(date.split('-')[1]);
	var day = Number(date.split('-')[2]);
	var hour = Number(time.split(':')[0]);
	var minute = Number(time.split(':')[1]);
	var second = 0;
	var localDate = new Date(year, month, day, hour, minute, second);
	return localDate.toUTCString();
}

function localToMSJSON(local)
{
	var date = local.split(' ')[0];
	var time = local.split(' ')[1];
	var year = Number(date.split('-')[0]);
	var month = Number(date.split('-')[1]) - 1;
	var day = Number(date.split('-')[2]);
	var hour = Number(time.split(':')[0]);
	var minute = Number(time.split(':')[1]);
	var second = 0;
	var localDate = new Date(year, month, day, hour, minute, second);
	return '/Date(' + localDate.getTime() + '+0000' + ')/';
}

function getLocalDateStringOfCurrent()
{
	var current = new Date();
	var offset = current.getTimezoneOffset();	
	return current.addMinutes(-offset).toISOString().split('T')[0];
}


function getLocalTimeStringOfCurrent()
{
	var current = new Date();
	var offset = current.getTimezoneOffset();
	return current.addMinutes(-offset).toISOString().split('T')[1].substr(0,5);
}