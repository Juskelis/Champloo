///container_foreach(script,arg1,arg2,...)
/*
 * Causes every item in calling container
   to execute given script with a list
   of parameters as arguments.
 * The actual enumeration and script calling
   is contained in method "foreach" of the container.
 * Note that script provided must refer to
   the parameters not with argument[N],
   but with ds_list_find_value(argument0, N)!
 */

var i, _params;

_params = ds_list_create();
for (i=1; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

script_execute(foreach, argument[0], _params);

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);