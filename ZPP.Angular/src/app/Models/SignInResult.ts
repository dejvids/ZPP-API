import { JsonWebToken } from './JsonWebToken';

export class SignInResult{
    Success : boolean;
    Message : string;
    Token : JsonWebToken;
}