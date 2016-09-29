//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ToolGood.ReadyGo.SqlParser.Tags;

//namespace ToolGood.ReadyGo.SqlParser
//{
//    public abstract class Statement
//    {

//        internal List<BaseTag> ConvertSql(List<TextPoint> points,int layer )
//        {
//            List<BaseTag> list = new List<BaseTag>();

//            for (int i = 0; i < points.Count; i++) {
//                var point = points[i];
//                if (point.Token== Token.Unknown) {




//                }



//            }
//            return list;
//        }
//        private BaseTag Visitor(TextPoint point,List<BaseTag> tags)
//        {
//            switch (point.Token) {
//                case Token.Unknown:
//                    break;
//                case Token.String:
//                    break;
//                case Token.Number:
//                    break;
//                case Token.Split:
//                    break;
//                case Token.Comment:
//                    break;
//                case Token.Parameters:
//                    break;
//                case Token.ParametersEnd:
//                    break;
//                case Token.Semicolon:
//                    break;
//                default:
//                    break;
//            }
//        }



//        private SelectTag ConvertToSelect


//    }
//}
