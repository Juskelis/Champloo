///argument0 - number of blood splats to make
var num_blood_splats = argument0;
for(var i = 0; i < num_blood_splats; i++)
{
    instance_create(x,y, obj_Blood);
}
