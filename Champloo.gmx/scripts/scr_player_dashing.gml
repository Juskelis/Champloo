scr_get_input();

vsp = clamp(
    vsp + grav*0.5*global.timescale,
    -maxgrav*global.timescale,
    maxgrav*global.timescale
);

hsp = clamp(
    hsp,
    -maxspeed*global.timescale,
    maxspeed*global.timescale
);

scr_move_collide();

if(place_meeting(x, y + 1, obj_Wall))
{
    state = States.Normal;
}
else if(place_meeting(x - 1,y,obj_Wall) || place_meeting(x + 1,y,obj_Wall))
{
    state = States.WallRiding;
}
else if(can_dash)
{
    state = States.InAir;
}
