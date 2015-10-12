///scr_apply_forces()
if(force_x != 0)
{
    if(sign(hsp) != sign(force_x))
        hsp = force_x * global.timescale;
    else
        hsp += force_x * global.timescale;
}

if(force_y != 0)
{
    if(sign(vsp) != sign(force_y))
        vsp = force_y * global.timescale;
    else
        vsp += force_y * global.timescale;
}
    
//decay forces
force_x = scr_approach(force_x, 0, force_decay);
force_y = scr_approach(force_y, 0, force_decay);
