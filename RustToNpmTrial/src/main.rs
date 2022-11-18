struct UserName {
    name: String,
    surname: String
}

fn main() -> () {
    let user = UserName 
    {
         name: String::from("Vector"),
          surname: String::from("Flower") 
    };
    println!("Hello fellow {} {}", user.name, user.surname);
}
