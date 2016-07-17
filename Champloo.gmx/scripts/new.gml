///new(object,arg1,arg2,...)
/*
 * Creates a new component, possibly providing
   additional parameters. These parameters are then
   passed to initialization script, which is method
   "init" of the newly created component.
 * Note that initialization script must refer to
   the parameters not with argument[N], but with
   ds_list_find_value(argument0, N)!
 */

var i, _params;

_params = ds_list_create();
for (i=1; i<argument_count; i++) {
    ds_list_add(_params, argument[i]);
    }

/****************
 * wrapped part *
 ****************/

var _object, _constructor;
var _result;    //the newly created instance, to be returned

_object = argument[0];
_constructor = get_method(_object, "init");
    
_result = instance_create(0, 0, _object);
if (_constructor != -1) {
    with (_result) { script_execute(_constructor, _params); }
    }

/***********
 * endwrap *
 ***********/

ds_list_destroy(_params);

return _result;
