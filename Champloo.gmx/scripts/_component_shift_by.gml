/*
 * Internal script actually shifting components.
 * Vector to shift by is passed in argument0.
 */

var _args;
_args = argument0;
 
x += ds_list_find_value(_args, 0);
y += ds_list_find_value(_args, 1);

if (is(par_container)) {
    script_execute(foreach, _component_shift_by, _args);
    }
