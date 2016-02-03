///scr_spawn_blood_explosion(num_splats)

if(global.blood_enabled)
{
    var num_blood_splats = argument0;
    var angle_step = 360/num_blood_splats;
    var angle_variance = angle_step/4;
    for(var i = 0; i < num_blood_splats; i++)
    {
        with(instance_create(x,y, obj_Blood))
        {
            var dir = i*angle_step + random_range(-angle_variance,angle_variance);
            hsp = lengthdir_x(spd*2, dir);
            vsp = lengthdir_y(spd*2, dir);
        }
    }
}
