export class Constants {
    public static host = 'http://localhost:5000';
    public static defaultGroupId = '6c3e9253-8db9-4c7d-aafc-12391cb7b1c8';
    public static defaultImage = 'testimage.JPG';

    public static path(path: string) { return Constants.host + path; }
    public static angularPath(path: string) { return Constants.host + '/#' + path; }
}
