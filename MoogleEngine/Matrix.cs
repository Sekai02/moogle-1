public class Matrix<T>
{
    private readonly T[,] _data;

    public Matrix(T[,] data)
    {
        _data = data;
    }

    public int Rows => _data.GetLength(0);
    public int Columns => _data.GetLength(1);

    public Matrix<T> Add(Matrix<T> other)
    {
        if (Rows != other.Rows || Columns != other.Columns)
            throw new ArgumentException("Matrices must be the same size");

        var result = new T[Rows, Columns];

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                result[i, j] = (dynamic)_data[i, j] + other._data[i, j];
            }
        }

        return new Matrix<T>(result);
    }

    public Matrix<T> Multiply(Matrix<T> other)
    {
        if (Columns != other.Rows)
            throw new ArgumentException("Matrices cannot be multiplied");

        var result = new T[Rows, other.Columns];

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < other.Columns; j++)
            {
                T sum = default;
                for (var k = 0; k < Columns; k++)
                {
                    sum += (dynamic)_data[i, k] * other._data[k, j];
                }
                result[i, j] = sum;
            }
        }

        return new Matrix<T>(result);
    }

    public Matrix<T> ProductByScalar(T scalar)
    {
        var result = new T[Rows, Columns];

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                result[i, j] = (dynamic)_data[i, j] * scalar;
            }
        }

        return new Matrix<T>(result);
    }

    public void Print()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Console.Write("{0} ", _data[i, j]);
            }
            Console.Write('\n');
        }
    }

    private double DeterminantOrder1()
    {
        return (dynamic)_data[0, 0];
    }

    private double DeterminantOrder2()
    {
        return (dynamic)_data[0, 0] * _data[1, 1] - (dynamic)_data[0, 1] * _data[1, 0];
    }

    private double DeterminantOrder3()
    {
        return (dynamic)_data[0, 0] * _data[1, 1] * _data[2, 2] + (dynamic)_data[0, 1] * _data[1, 2] * _data[2, 0] + (dynamic)_data[1, 0] * _data[2, 1] * _data[0, 2]
        - (dynamic)_data[0, 2] * _data[1, 1] * _data[2, 0] - (dynamic)_data[0, 1] * _data[1, 0] * _data[2, 2] - (dynamic)_data[1, 2] * _data[2, 1] * _data[0, 0];
    }

    public double Determinant()
    {
        if (Rows != Columns) throw new ArgumentException("Not a square matrix");
        if (Rows == 3) return DeterminantOrder3();
        if (Rows == 2) return DeterminantOrder2();
        if (Rows == 1) return DeterminantOrder1();
        else throw new ArgumentException("Determinants of higher orders aren't still implemented");
    }
}