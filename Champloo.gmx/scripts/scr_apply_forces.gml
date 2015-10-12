///scr_apply_forces()
hsp += force_x * global.timescale;
vsp += force_y * global.timescale;
    
//decay forces
force_x = clamp(scr_approach(force_x, 0, force_decay), -10,10);
force_y = clamp(scr_approach(force_y, 0, force_decay), -10,10);
