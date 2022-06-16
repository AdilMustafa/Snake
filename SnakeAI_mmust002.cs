using UnityEngine;

using AlanZucconi.AI.BT;
using AlanZucconi.Snake;

using System.Linq;

[CreateAssetMenu
(
    fileName = "SnakeAI_mmust002",
    menuName = "Snake/2021-22/SnakeAI_mmust002"
)]
public class SnakeAI_mmust002 : SnakeAI
{
    public override Node CreateBehaviourTree(SnakeGame Snake)
    {

        return new Selector
        (
            new Selector
            (
                //if an obstacle is onfront of the snakes head and to its left the snake will turn right
                new Sequence
                (
                    new Condition(Snake.IsObstacleAhead),
                    new Condition(Snake.IsObstacleLeft),
                    new Action(Snake.TurnRight)
                ),
            
                //if an obstacle is onfront of the snakes head and to its right the snake will turn left
                new Sequence
                (
                    new Condition(Snake.IsObstacleAhead),
                    new Condition(Snake.IsObstacleRight),
                    new Action(Snake.TurnLeft)
                )
            ),
            


            new Selector
            (   
                //if the food is 4 or less blocks to the left, the snake turns left 
                new Sequence
                (
                    new Condition(() => Snake.RaycastLeft().Distance <= 5),
                    new Condition(() => Snake.RaycastLeft().Cell == SnakeGame.Cell.Food),
                    new Action(Snake.TurnLeft)
                ),

                //if the food is 4 or less blocks to the left, the snake turns left 
                new Sequence
                (   
                    new Condition(() => Snake.RaycastRight().Distance <= 5),
                    new Condition(() => Snake.RaycastRight().Cell == SnakeGame.Cell.Food),
                    new Action(Snake.TurnRight)
                )
            ), 

           


            new Selector
            ( 
                
                new Filter
                (
                    //checks if a wall is onfront of snake's head, also checks if the amount of space on the left and right are the same
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft(), dist_right = Snake.RaycastRight();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.None &&
                        dist_left.Distance == dist_right.Distance;
                    },

                    //will randomly move left or right
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnRight)
                    )     
                ),

                
                new Filter
                (
                    //checks if the snake is facing a wall and checks if there is more free space at the left than right
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_right = Snake.RaycastRight(), dist_left = Snake.RaycastLeft();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.None && 
                        dist_left.Distance > dist_right.Distance;
                    },

                    //it is more likely to turn left than right (this is to prevent the snake from looping)
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnRight)
                    )   
                ),

               
                new Filter
                (
                    //checks if the snake is facing a wall and checks if the left side has less space than the right
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_right = Snake.RaycastRight(), dist_left = Snake.RaycastLeft();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.None && 
                        dist_left.Distance < dist_right.Distance;
                    },

                    //it is more likely to turn right than left (this is to prevent the snake from looping)
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnLeft)
                    ) 
                )
            ),

            new Selector
            (

                new Filter
                (
                    //checks if the snake is facing its body & check if the snake is able to reach its tale's position
                    () =>
                        {
                            SnakecastHit hit = Snake.RaycastAhead();
                            return
                            hit.Distance <=3 &&
                            hit.Cell == SnakeGame.Cell.Snake && 
                            Snake
                            .AvailableNeighbours(Snake.TailPosition)
                            .Any(position => Snake.IsReachable(position));
                        },
                    
                    //snake will move towards the position of its tail using pathfinding
                    new Action
                    (
                        () => Snake.MoveTowards
                        (
                            Snake
                            .AvailableNeighbours(Snake.TailPosition)
                            .FirstOrDefault(position => Snake.IsReachable(position))
                        )
                    )
                ),
                

                new Filter
                (
                    //used to make space when swelling into iself
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_right = Snake.RaycastRight(),dist_left = Snake.RaycastLeft();
                        return
                        hit.Distance <= 1 &&
                        hit.Cell == SnakeGame.Cell.Snake &&
                        dist_right.Distance <= 1 &&
                        dist_right.Cell == SnakeGame.Cell.Snake &&
                        dist_left.Distance <= 1 &&
                        dist_left.Cell == SnakeGame.Cell.Snake;
                    },  
                    new Action(Snake.TurnLeft)
                ),

                new Filter
                (
                    //used to make space when swelling into iself
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft();
                        return
                        hit.Distance <=1 &&
                        hit.Cell == SnakeGame.Cell.Snake &&
                        dist_left.Distance <= 1 &&
                        dist_left.Cell == SnakeGame.Cell.Snake;
                    },  
                    new Action(Snake.TurnRight)
                ),

                new Filter
                (
                    //used to make space when swelling into iself
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_right = Snake.RaycastRight();
                        return
                        hit.Distance <=1 &&
                        hit.Cell == SnakeGame.Cell.Snake &&
                        dist_right.Distance <= 1 &&
                        dist_right.Cell == SnakeGame.Cell.Snake;
                    },  
                    new Action(Snake.TurnLeft)
                )
            ), 
            
            new Selector
            (
                new Filter
                (
                    //checks if its body is onfront of snake's head, also checks if the amount of space on the left and right are the same
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft(), dist_right = Snake.RaycastRight();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.Snake && 
                        dist_left.Distance == dist_right.Distance;
                    },
                    
                    //will randomly move left or right
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnLeft)
                    )
                ),

                new Filter
                (
                    //checks if the snake is facing its body and checks if there is less free space at the left than right
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft(), dist_right = Snake.RaycastRight();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.Snake && 
                        dist_left.Distance < dist_right.Distance;
                    },
                    
                    //snake will more likely turn right
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnRight),
                        new Action(Snake.TurnLeft)
                    )
                ),

                new Filter
                (
                    //checks if the snake is facing its body and checks if there is more free space at the left than right
                    () =>
                    {
                        SnakecastHit hit = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft(), dist_right = Snake.RaycastRight();
                        return
                        hit.Distance == 0 &&
                        hit.Cell == SnakeGame.Cell.Snake && 
                        dist_left.Distance > dist_right.Distance;
                    },
                    
                    //snake will more likely turn left
                    new Selector
                    (
                        true,
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnLeft),
                        new Action(Snake.TurnRight)
                    )
                )
            ),



            new Filter
            (
                //checks if the food is near the body of the snake, if so it will follow its tail until it can reach it
                () =>
                {
                    SnakecastHit hit_food = Snake.RaycastAhead(),hit_snake = Snake.RaycastAhead(), dist_left = Snake.RaycastLeft();
                    return
                    hit_food.Distance <= 5 &&
                    hit_food.Cell == SnakeGame.Cell.Food &&
                    hit_snake.Distance == hit_food.Distance + 1 &&
                    hit_snake.Cell == SnakeGame.Cell.Snake &&  
                    dist_left.Distance == 0 &&
                    dist_left.Cell == SnakeGame.Cell.Snake && 
                    Snake
                    .AvailableNeighbours(Snake.TailPosition)
                    .Any(position => Snake.IsReachable(position));
                },

                //snake will follow its tale
                new Action
                (
                    () => Snake.MoveTowards
                    (
                        Snake
                        .AvailableNeighbours(Snake.TailPosition)
                        .FirstOrDefault(position => Snake.IsReachable(position))
                    )
                )
            ),

            
            
            //if the food is reachable the snake will use pathfinding to reach it
            new Filter
            (
                new Condition(Snake.IsFoodReachable),
                new Action(Snake.MoveTowardsFood)
            )
        );
    }
}
