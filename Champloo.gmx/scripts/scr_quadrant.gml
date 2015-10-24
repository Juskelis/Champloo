///scr_quadrant(obj_b, error_window)
/*
    returns where we are relative to b from 0 to 8
    with 0 being us above b, and moving clockwise around
    
    error window is a radius value (so 5 will have a total window of 10)
*/


var obj_b = argument0;
var error = abs(argument1);

if(obj_b.y - bbox_bottom > error)
{
    if(obj_b.x - bbox_left > 0)
        return 8;
    else if(bbox_right - obj_b.x > 0)
        return 1;
    
    return 0;
}
else if(bbox_top - obj_b.y > error)
{
    if(obj_b.x - bbox_left > 0)
        return 5;
    else if(bbox_right - obj_b.x > 0)
        return 3;
    
    return 4;
}
else
{
    if(obj_b.x - bbox_left > error)
        return 7;
    else if(bbox_right - obj_b.x > error)
        return 2;
    
    return -1;
}
