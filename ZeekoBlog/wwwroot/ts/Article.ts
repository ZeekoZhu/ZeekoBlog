export class Article implements IArticle {
    id?: number;
    title: string = '';
    summary: string = '';
    content: string = '';
    lastEdited: Date = new Date();

    constructor(obj?: IArticle) {
        Object.assign(this, obj);
    }
}

interface IArticle {
    id?: number;
    title?: string;
    summary?: string;
    content?: string;
    lastEdited?: Date;
}
