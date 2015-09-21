///scr_approach_angle(start_angle, end_angle, amount)

if(scr_clockwise_angle(argument0, argument1))
{
    var applied_angle = argument0 - argument2;
    
    if(!scr_clockwise_angle(applied_angle, argument1))
        return argument1;
    
    return applied_angle;
}
else
{
    var applied_angle = argument0 + argument2;
    
    if(scr_clockwise_angle(applied_angle, argument1))
        return argument1;
    
    return applied_angle;
}
