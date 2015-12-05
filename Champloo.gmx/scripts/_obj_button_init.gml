/*
    obj_button constructor(label, on_pressed_call, [on_pressed_args] )
*/

var _args = argument0;

//build arguments list (if there are arguments)
if(ds_list_empty(_args))
{
    err("In _par_button_init, need to have at least two arguments!");
    exit;
}

label = ds_list_find_value(_args, 0);
if(string_width(label) > width)
{
    width = string_width(label);
}
if(string_height(label) > height)
{
    height = string_height(label);
}

if(sprite_index != -1)
{
    image_xscale = width/sprite_width;
    image_yscale = height/sprite_height;
}

on_press = ds_list_find_value(_args, 1);
if(ds_list_size(_args) > 1) {
    for(var i = 2; i < ds_list_size(_args); i++) {
        on_press_args[i-2] = ds_list_find_value(_args, i);
    }
}
