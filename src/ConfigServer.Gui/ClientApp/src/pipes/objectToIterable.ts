import { Pipe, PipeTransform } from '@angular/core';
@Pipe({ name: 'toIterator' })
export class ObjectToIteratorPipe implements PipeTransform {
    public transform(value: any, args: string[]): any {
        const keys = Object.keys(value);
        const array = new Array<any>();
        for (const key of keys) {
            const val = value[key];
            array.push(val);
        }
        return array;
    }
}
