/*
    obj_button constructor(label, font, color, on_pressed_call, [on_pressed_args] )
*/

var _args = argument0;

//build arguments list (if there are arguments)
if(ds_list_empty(_args))
{
    err("Missing arguments!");
    exit;
}

label = ds_list_find_value(_args, 0);
font = ds_list_find_value(_args, 1);
color = ds_list_find_value(_args,2);
draw_set_font(font);
if(string_width(label) > width)
{
    width = string_width(label) + 20;
}
if(string_height(label) > height)
{
    height = string_height(label) + 6;
}

if(sprite_index != -1)
{
    image_xscale = width/sprite_width;
    image_yscale = height/sprite_height;
}

on_press = ds_list_find_value(_args, 3);
if(ds_list_size(_args) > 3) {
    for(var i = 4; i < ds_list_size(_args); i++) {
        on_press_args[i-4] = ds_list_find_value(_args, i);
    }
}
