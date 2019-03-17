export class User{
    id : number;
    login : string;
    email : string;
    name : string;
    surname : string;
    password : string;

    constructor (login?: string, name?:string, surname?:string, email? :string){
        this.login = login;
        this.name = name;
        this.surname = surname;
        this.email = email;
    }
}