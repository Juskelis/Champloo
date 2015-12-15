///is(object, object_to_test = noone)
/*
 * Returns whether calling instance is of a given
   object type.
 * It's the same as either being such object
   directly, or having that type as a parent.
 */

if(argument_count > 1)
    return argument[1].object_index == argument[0] || object_is_ancestor(argument[1].object_index, argument[0]);
return (object_index == argument[0] || object_is_ancestor(object_index, argument[0]));