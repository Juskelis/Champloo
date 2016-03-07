///scr_cubic_lerp(start, end, speed)

var s = argument0;
var e = argument1;
var d = argument2;

return d*d*d*(e-s)/2 + s;
