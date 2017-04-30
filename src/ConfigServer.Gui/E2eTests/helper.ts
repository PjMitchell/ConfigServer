
export function asyncHelper(done: DoneFn, func: (d: DoneFn) => Promise<void>) {
    func(done).then(() => done()).catch((reason) => {
        fail(reason);
        done();
    });
}
