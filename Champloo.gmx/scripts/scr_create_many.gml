//argument0 : object to create
//argument1 : min x
//argument2 : min y
//argument3 : max x
//argument4 : max y
//argument5 : how many

for(var i = 0; i < argument5; i++)
{
    instance_create(
        random_range(argument1, argument3),
        random_range(argument2, argument4),
        argument0
    );
}
