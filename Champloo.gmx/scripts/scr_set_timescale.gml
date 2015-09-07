// argument0 - target timescale
// argument1 - time to reach the target (easing)

with(obj_Timescale)
{
    target_timescale = argument0;
    if(argument1 > 0)
    {
        timescale_delta_step = (target_timescale - global.timescale)/(argument1*room_speed);
    }
    else
    {
        timescale_delta_step = 0;
    }
}