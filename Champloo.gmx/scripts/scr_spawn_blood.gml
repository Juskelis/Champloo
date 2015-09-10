///argument0 - number of blood splats to make
///argument1 - direction to "aim" them in
///argument2 - scatter from direction
var num_blood_splats = argument0;
var blood_inst;
for(var i = 0; i < num_blood_splats; i++)
{
    blood_inst = instance_create(x,y, obj_Blood);
    with(blood_inst)
    {
        var dir = random_range(argument1 - argument2, argument1 + argument2);
        hsp = lengthdir_x(spd, dir);
        vsp = lengthdir_y(spd*2, dir);
    }
}
