import { Pipe, PipeTransform } from '@angular/core';
@Pipe({ name: 'toIterator' })
export class ObjectToIteratorPipe implements PipeTransform {
    transform(value: any, args: string[]): any {
        var keys = Object.keys(value);
        var array = new Array<any>();
        for (var i = 0; i < keys.length; i++) {
            var val = value[keys[i]];
            array.push(val);
        }
        return array;
    }
}

@Pipe({ name: 'toKeyValuePairs' })
export class ObjectToKeyValuePairsPipe implements PipeTransform {
    transform(value: any, args: string[]): any {
        var keys = Object.keys(value);
        var array = new Array<any>();
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            var val = value[key];
            array.push({ 'key' : key, 'value' : val});
        }
        return array;
    }
}