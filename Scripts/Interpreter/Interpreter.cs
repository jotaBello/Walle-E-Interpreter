using System;
using System.Collections.Generic;
using Godot;
public class Interpreter
{
    private Environment environment = new Environment();

    public void interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
            {
                execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Compiler.runtimeError(error);
        }
    }
    
    private void execute(Stmt stmt)
    {
        if(stmt !=null)
        stmt.accept(this);
    }

    

    
    public Object visitLiteralExpr(Literal expr)
    {
        return expr.value;
    }
    public Object visitGroupingExpr(Grouping expr)
    {
        return evaluate(expr.expression);
    }
    private Object evaluate(Expr expr)
    {
        return expr.accept(this);
    }


    public void visitExpressionStmt(ExpressionStmt stmt)
    {
        evaluate(stmt.expression);
    }

    public void visitPrintStmt(PrintStmt stmt)
    {
        Object value = evaluate(stmt.expression);
        //TEMPORAL
        GD.Print(value);
    }

    public void visitVarStmt(VarStmt stmt)
    {
        Object value = null;

        if (stmt.expression != null)
        {
            value = evaluate(stmt.expression);
        }

        environment.define(stmt.name.lexeme, value);
    }
    public Object visitVariableExpr(Variable expr)
    {
        return environment.get(expr.name);
    }

    public void visitBlockStmt(BlockStmt stmt)
    {
        executeBlock(stmt.statements, new Environment(environment));
    }

     void executeBlock(List<Stmt> statements,Environment environment)
    {
        Environment previous = this.environment;
        try
        {
            this.environment = environment;
            foreach (Stmt statement in statements)
            {
                execute(statement);
            }
        }
        finally
        {
            this.environment = previous;
        }
    }



















    public Object visitUnaryExpr(Unary expr)
    {
        Object right = evaluate(expr.right);
        switch (expr.operation.type)
        {
            case TokenType.MINUS:
                checkNumberOperand(expr.operation, right);
                return -(int)right;
            case TokenType.BANG:
                return !isTruthy(right);
        }

        return null;
    }

    public Object visitBinaryExpr(Binary expr)
    {
        Object left = evaluate(expr.left);
        Object right = evaluate(expr.right);
        switch (expr.operation.type)
        {
            case TokenType.GREATER:
                checkNumberOperand(expr.operation, left, right);
                return (int)left > (int)right;
            case TokenType.GREATER_EQUAL:
                checkNumberOperand(expr.operation, left, right);
                return (int)left >= (int)right;
            case TokenType.LESS:
                checkNumberOperand(expr.operation, left, right);
                return (int)left < (int)right;
            case TokenType.LESS_EQUAL:
                checkNumberOperand(expr.operation, left, right);
                return (int)left <= (int)right;

            case TokenType.BANG_EQUAL: return !isEqual(left, right);
            case TokenType.EQUAL_EQUAL: return isEqual(left, right);


            case TokenType.MINUS:
                checkNumberOperand(expr.operation, left, right);
                return (int)left - (int)right;
            case TokenType.SLASH:
                checkNumberOperand(expr.operation, left, right);
                return (int)left / (int)right;
            case TokenType.STAR:
                checkNumberOperand(expr.operation, left, right);
                return (int)left * (int)right;
            case TokenType.PLUS:
                checkNumberOperand(expr.operation, left, right);
                return (int)left + (int)right;
        }


        return null;
    }





    private bool isEqual(Object a, Object b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return a.Equals(b);
    }


    private bool isTruthy(Object objec)
    {
        if (objec == null) return false;
        if (objec is bool) return (bool)objec;
        return true;
    }

    private void checkNumberOperand(Token operation, Object operand)
    {
        if (operand is int) return;
        throw new RuntimeError(operation, "Operand must be a number.");
    }
    private void checkNumberOperand(Token operation,Object left, Object right) 
    {
        if (left is int && right is int) return;

        throw new RuntimeError(operation, "Operands must be numbers.");
    }
 
 


 
} 


