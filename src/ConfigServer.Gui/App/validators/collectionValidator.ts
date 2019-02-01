import { FormArray, ValidatorFn } from "@angular/forms";

export function uniqueKey(uniqueKeyProperty: string): ValidatorFn {
    return (control: FormArray): {[key: string]: any} => {
        const keyCount = {};
        control.controls.forEach((value) => {
            const keyValue = value.value[uniqueKeyProperty];
            if (keyCount[keyValue]) {
                keyCount[keyValue] = keyCount[keyValue] + 1;
            } else {
                keyCount[keyValue] = 1;
            }
        });
        const duplicateKeys = Object.keys(keyCount).filter((prop) => keyCount[prop] !== 1);
        return duplicateKeys.length === 0 ? null : { duplicate : duplicateKeys};
    };
}

export function uniqueItem(): ValidatorFn {
    return (control: FormArray): {[key: string]: any} => {
        const keyCount = {};
        control.controls.forEach((value) => {
            const keyValue = value.value;
            if (keyCount[keyValue]) {
                keyCount[keyValue] = keyCount[keyValue] + 1;
            } else {
                keyCount[keyValue] = 1;
            }
        });
        const duplicateKeys = Object.keys(keyCount).filter((prop) => keyCount[prop] !== 1);
        return duplicateKeys.length === 0 ? null : { duplicate : duplicateKeys};
    };
  }
