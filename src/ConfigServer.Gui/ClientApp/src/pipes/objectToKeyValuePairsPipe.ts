import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'toKeyValuePairs' })
export class ObjectToKeyValuePairsPipe implements PipeTransform {
    public transform(value: any, args: string[]): any {
        const keys = Object.keys(value);
        const array = new Array<any>();
        for (const key of keys) {
            const val = value[key];
            array.push({ key, value : val});
        }
        return array;
    }
}
