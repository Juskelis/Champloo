///component_shift_by(x,y)
/*
 * Shifts a omponent by a given x/y vector.
 * If the component is a container, all its
   items are shifted by the same vector.
 */

//don't do anything if nothing changes
if (argument0 == 0 && argument1 == 0)
    exit;

var _vector;
_vector = ds_list_create();
ds_list_add(_vector, argument0);
ds_list_add(_vector, argument1);

_component_shift_by(_vector);

ds_list_destroy(_vector);
