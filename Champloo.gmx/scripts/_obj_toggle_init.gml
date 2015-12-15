/*
    obj_toggle constructor( label, font, color, enabled, on_pressed_call, [on_pressed_args] )
*/

var _args = argument0;

if(ds_list_empty(_args))
{
    err("Need to have at least one argument!");
    exit;
}

label = ds_list_find_value(_args, 0);
font = ds_list_find_value(_args, 1);
color = ds_list_find_value(_args, 2);
active = ds_list_find_value(_args, 3);
draw_set_font(font);
height = max(height, string_height(label));
width = max(width, string_width(label) + height);

on_press = ds_list_find_value(_args, 4);
if(ds_list_size(_args) > 4) {
    for(var i = 5; i < ds_list_size(_args); i++) {
        on_press_args[i-5] = ds_list_find_value(_args, i);
    }
}
