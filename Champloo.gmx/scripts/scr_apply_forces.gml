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

if(dash_force_x != 0)
{
    if(sign(hsp) != sign(dash_force_x))
        hsp = dash_force_x * global.timescale;
    else
        hsp += dash_force_x * global.timescale;
}

if(dash_force_y != 0)
{
    if(sign(vsp) != sign(dash_force_y))
        vsp = dash_force_y * global.timescale;
    else
        vsp += dash_force_y * global.timescale;
}
    
//decay forces
force_x = scr_approach(force_x, 0, force_decay);
force_y = scr_approach(force_y, 0, force_decay);
dash_force_x = scr_approach(dash_force_x, 0, dash_decay);
dash_force_y = scr_approach(dash_force_y, 0, dash_decay);
